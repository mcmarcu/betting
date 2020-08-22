__I Basic league workflow__


1. Get best metrics plain
```
-e --league=Championship --year=2018 --matchday=max-2 --mreverse=10 --minodds=1.0 --maxodds=2.0 --minyearprofit=0 --successrate=70 --betstyle=all --loglevel=LOG_RESULT
```
* [outputPremierLeague](./OutputData/dataI1PremierLeague.txt)
* [outputChampionship](./OutputData/dataI1Championship.txt)
* [output2Bundesliga](./OutputData/dataI12Bundesliga.txt)
* [outputSerieB](./OutputData/dataI1SerieB.txt)
* [outputBelgium](./OutputData/dataI1Belgium.txt)

2. Update data with one of the metrics (inspectMetric ignored, only required for fair odds)
```
-u -x=76 --league=Championship -y=2018 -r=9
```
3. Get best metric with updated data (where opponent CoeficientWeight is 3)
```
-e --useExpanded=3 --league=Championship --year=2018 --matchday=max-2 --mreverse=10 --minodds=1.0 --maxodds=2.0 --minyearprofit=0 --successrate=70 --betstyle=all --loglevel=LOG_RESULT
```
* [outputPremierLeague](./OutputData/dataI3PremierLeague.txt)
* [outputChampionship](./OutputData/dataI3Championship.txt)
* [output2Bundesliga](./OutputData/dataI32Bundesliga.txt)
* [output2SerieB](./OutputData/dataI3SerieB.txt)

4. Test this metric to get more info
```
-e --useExpanded=3 -x=39000 --league=Championship --year=2018 --matchday=max-2 --mreverse=10 --minodds=1.0 --maxodds=2.0 --minyearprofit=0 --successrate=70 --betstyle=all --loglevel=LOG_INFO
```


__II Fair odds workflow__
1. Find best metrics for fair odds
```
-u -e --league=Championship -y=2018 -r=9
```
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