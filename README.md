__I Basic league workflow__


1. Get best metrics plain
```
-e --league=Championship --year=2018 --matchday=max-2 --mreverse=10 --minodds=1.0 --maxodds=2.0 --minyearprofit=0 --betstyle=all --loglevel=LOG_RESULT
```
* [outputPremierLeague](./OutputData/dataI1PremierLeague.txt)
* [outputChampionship](./OutputData/dataI1Championship.txt)
* [outputBundesliga](./OutputData/dataI1Bundesliga.txt)
* [output2Bundesliga](./OutputData/dataI12Bundesliga.txt)
* [outputSerieA](./OutputData/dataI1SerieA.txt)
* [outputSerieB](./OutputData/dataI1SerieB.txt)
* [outputFrance](./OutputData/dataI1France.txt)
* [output2France](./OutputData/dataI12France.txt)
* [outputPrimera](./OutputData/dataI1Primera.txt)
* [outputSegunda](./OutputData/dataI1Segunda.txt)
* [outputBelgium](./OutputData/dataI1Belgium.txt)
* [outputScotland](./OutputData/dataI1Scotland.txt)
* [outputGreece](./OutputData/dataI1Greece.txt)

2. Update data with one of the metrics (inspectMetric ignored, only required for fair odds)
```
-u -x=76 --league=Championship -y=2018 -r=9
```
3. Get best metric with updated data (where opponent CoeficientWeight is 3)
```
-e --useExpanded=3 --league=Championship --year=2018 --matchday=max-2 --mreverse=10 --minodds=1.0 --maxodds=2.0 --minyearprofit=0  --betstyle=all --loglevel=LOG_RESULT
```
* [outputPremierLeague](./OutputData/dataI3PremierLeague.txt)
* [outputChampionship](./OutputData/dataI3Championship.txt)
* [outputBundesliga](./OutputData/dataI3Bundesliga.txt)
* [output2Bundesliga](./OutputData/dataI32Bundesliga.txt)
* [outputSerieA](./OutputData/dataI3SerieA.txt)
* [outputSerieB](./OutputData/dataI3SerieB.txt)
* [outputFrance](./OutputData/dataI3France.txt)
* [output2France](./OutputData/dataI32France.txt)
* [outputPrimera](./OutputData/dataI3Primera.txt)
* [outputSegunda](./OutputData/dataI3Segunda.txt)
* [outputBelgium](./OutputData/dataI3Belgium.txt)
* [outputScotland](./OutputData/dataI3Scotland.txt)
* [outputGreece](./OutputData/dataI3Greece.txt)

4. Test this metric to get more info
```
-e --useExpanded=3 -x=39000 --league=Championship --year=2018 --matchday=max-2 --mreverse=10 --minodds=1.0 --maxodds=2.0 --minyearprofit=0 --betstyle=all --loglevel=LOG_INFO
```


__II Fair odds workflow__
1. Find best metrics for fair odds
```
-u -e --league=Championship -y=2018 -r=9
```

* [outputChampionship](./OutputData/dataII1Championship.txt)
* [outputSerieA](./OutputData/dataII1SerieA.txt)
* [outputBundesliga](./OutputData/dataII1Bundesliga.txt)


2. Update data with fair odds
```
-u -x=76 --league=Championship -y=2018 -r=9
```
3. Todo

__III Use data to predict__

1. Predict using plain data
```
-w --useExpanded=3 -x=39000 --league=Championship --year=2019 --matchday=37 --minodds=1.0 --maxodds=2.0 --betstyle=all --loglevel=LOG_INFO
```

2. Predict using expanded CoeficientWeight 3 data
```
-w --useExpanded=3 -x=39000 --league=Championship --year=2019 --matchday=37 --minodds=1.0 --maxodds=2.0 --betstyle=all --loglevel=LOG_INFO
```


__IV Successfull runs__

1. Championship
```
-x=37006 --useExpanded=3 --league=Championship --year=2018 --matchday=max-2 --mreverse=10 --minodds=1.0 --maxodds=2 --minyearprofit=0 --betstyle=all --loglevel=LOG_INFO
Result True, Rate 74.88, avgProfit 501.70
```


```
-x=68078 --useExpanded=3 --league=Championship --year=2018 --matchday=max-2 --mreverse=10 --minodds=1.0 --maxodds=2 --minyearprofit=0 --betstyle=all --loglevel=LOG_INFO
Result True, Rate 77.75, avgProfit 275.33
```

2. Bundesliga

```
-x=55700 --league=Bundesliga --year=2018 --matchday=max-2 --mreverse=10 --minodds=1.0 --maxodds=2.0 --minyearprofit=0 --betstyle=all --loglevel=LOG_INFO
Result True, Rate 87.38, avgProfit 47.70
```

3. 2Bundesliga

```
-x=90900 --useExpanded=3 --league=2Bundesliga --year=2018 --matchday=max-2 --mreverse=10 --minodds=1.0 --maxodds=2 --minyearprofit=0 --betstyle=all --loglevel=LOG_INFO
Result True, Rate 76.82, avgProfit 276.90
```

