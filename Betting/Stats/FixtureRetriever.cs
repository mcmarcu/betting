using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Betting.DataModel;
using System.Net;
using System.IO;
using Betting.Config;
using System.Web.Script.Serialization;
using System.Data.SqlClient;

namespace Betting.Stats
{
    class FixtureRetriever
    {

        private static void AddDataToDB(string url,string data)
        {
            var insertSQL = "INSERT INTO sitedata (url, data) VALUES (@URL, @DATA)";
            url = url.Replace("https://api.football-data.org/v1/", "");

            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\mcmar\\documents\\visual studio 2017\\Projects\\Betting\\Betting\\DB\\cachedata.mdf\";Integrated Security=True";
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(insertSQL, cn))
            {
                cn.Open();

                cmd.Parameters.AddWithValue("@URL", url);
                cmd.Parameters.AddWithValue("@DATA", data);
                cmd.ExecuteNonQuery();
            }
        }

        private static bool GetDataFromDB(string url, out string data)
        {
            url = url.Replace("https://api.football-data.org/v1/", "");
            var selectSQL = "SELECT * FROM sitedata where url=@URL";

            try
            {
                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\mcmar\\documents\\visual studio 2017\\Projects\\Betting\\Betting\\DB\\cachedata.mdf\";Integrated Security=True";
                using (var cn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(selectSQL, cn))
                {
                    cn.Open();

                    cmd.Parameters.AddWithValue("@URL", url);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            data = reader[1].ToString();
                            return true;
                        }
                        else
                        {
                            data = "";
                            return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                data = "";
                return false;
            }
        }

        private static string GetHtmlData(string url, bool useCache = true)
        {
            
            string html = string.Empty;
            if (useCache && GetDataFromDB(url, out html))
                return html;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            if(useCache)
                AddDataToDB(url, html);

            return html;
        }

        private static int GetLeagueId(int year)
        {
            string leagueName = ConfigManager.Instance.GetLeagueName();
            string templateUrl = ConfigManager.Instance.GetCompetitionsUrl();
            string competitionsUrl = String.Format(templateUrl, year);
            string response = GetHtmlData(competitionsUrl);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic leagues = serializer.DeserializeObject(response);
            foreach(dynamic league in leagues)
            {
                if (league["caption"].IndexOf(leagueName) == 0)
                    return league["id"];
            }
            throw new KeyNotFoundException();
        }

        private static int GetNumberOfMatchdays(int year)
        {
            string templateUrl = ConfigManager.Instance.GetLeagueInfoUrl();
            string infoUrl = String.Format(templateUrl, GetLeagueId(year));
            string response = GetHtmlData(infoUrl);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic info = serializer.DeserializeObject(response);
            return info["numberOfMatchdays"];
        }

        public static List<Fixture> GetRound(int year, int matchDay)
        {
            List<Fixture> result = new List<Fixture>();
            string templateUrl = ConfigManager.Instance.GetFixturesForLeagueUrl();
            string fixturesUrl = String.Format(templateUrl, GetLeagueId(year), matchDay);
            bool useCache = year < DateTime.Now.Year || matchDay < GetCurrentMatchDay(year);
            string response = GetHtmlData(fixturesUrl, useCache);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic fixtures = serializer.DeserializeObject(response);
            foreach (dynamic fixture in fixtures["fixtures"])
            {
                Fixture newFixture = new Fixture();
                newFixture.homeTeamName = fixture["homeTeamName"];
                newFixture.awayTeamName = fixture["awayTeamName"];
                newFixture.homeTeamId = Int32.Parse(fixture["_links"]["homeTeam"]["href"].Replace("http://api.football-data.org/v1/teams/", ""));
                newFixture.awayTeamId = Int32.Parse(fixture["_links"]["awayTeam"]["href"].Replace("http://api.football-data.org/v1/teams/", ""));
                newFixture.finished = fixture["status"] == "FINISHED";
                try
                {
                    newFixture.finalScore.homeTeamGoals = fixture["result"]["goalsHomeTeam"];
                    newFixture.finalScore.awayTeamGoals = fixture["result"]["goalsAwayTeam"];
                    newFixture.halfScore.homeTeamGoals = fixture["result"]["halfTime"]["goalsHomeTeam"];
                    newFixture.halfScore.awayTeamGoals = fixture["result"]["halfTime"]["goalsAwayTeam"];
                    newFixture.date = DateTime.Parse(fixture["date"]);
                }
                catch(Exception)
                {}

                result.Add(newFixture);
            }
            return result;
        }

        public static void GetPrevRound(out int outYear, out int outDay, int currentYear, int currentDay)
        {
            if (currentDay == 1)
            {
                outDay = GetNumberOfMatchdays(currentYear - 1);
                outYear = currentYear - 1;
            }
            else
            {
                outDay = currentDay - 1;
                outYear = currentYear;
            }  
        }

        public static int GetCurrentMatchDay(int year)
        {
            string templateUrl = ConfigManager.Instance.GetLeagueInfoUrl();
            string infoUrl = String.Format(templateUrl, GetLeagueId(year));
            string response = GetHtmlData(infoUrl, false);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic info = serializer.DeserializeObject(response);
            return info["currentMatchday"];
        }
    }
}
