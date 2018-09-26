# DonjorNight-Autominer
A mining tool that auto switches to the most profitable coins in the CryptoNight world. Uses cast-xmr. Windows only.

### Auto Restart and monitor:

Run this code as a batch file if you have stablity issues on your rig:
```
call:myDosFunc

:myDosFunc
	START /I DonjorNight-Autominer
	TIMEOUT 7400
	TASKKILL /IM DonjorNight-Autominer.exe /F /T
	TASKKILL /IM chromedriver.exe /F /T
	TASKKILL /IM cast_xmr-vega.exe /F /T
	TASKKILL /IM chrome.exe /F /T
call:myDosFunc

goto:eof
```

### How to use:
Edit the `DonjorNight-Coin.txt` file to set your pools and addresses using the format:

>`Coin Name, Coin Ticker, Cast-xmr algo#, pool, address,`

Eg: 
>`Haven, XHV, 7, haven.miner.rocks:4005, hvi1aCqoAZF19J8pijvqnrUkeAeP8Rvr4XyfDMGJcarhbL15KgYKM1hN7kiHMu3fer5k8JJ8YRLKCahDKFgLFgJMYAfnFbkERic2irNC1zEup,`

Cast-Xmr algos:
```
0 = CryptoNight (ASICS - Probs no good for gpu mining) 
1 = CryptoNightV7
2 = CryptoNight-Heavy
3 = CryptoNight-Lite
4 = CryptoNightV7-Lite
5 = CryptoNightIPBC-Lite/TUBE-Heavy
6 = CryptoNightXTL
7 = CryptoNightXHV-Heavy
8 = CryptoNight-Fast
```

All lines starting with * are ignored in the file.

##### Static Difficulty:
- Static diff is automatically applied after benchmarking based on hashrate * 30.
- currently only supports `address.diff` format - make sure you use a pool that supports this format.

### How it works:
The software reads the `DonjorNight-Coins.txt` file.

#### Benchmarking:
The software benchmarks each unique algorithm that could be used.
The benchmarking function will stop once the process has elapsed 45 seconds and the increase in average hash rate has increased by < 1%
This process is time consuming and in my testing the benchmarked rate is a slightly lower... Difficult to improve without extending the benchmark time.

At the moment, this benchmarking function fires every start-up and the benchmarked values are held are only held in memory. 

#### Finding the profitability rates:
Using a Selenium Chrome driver, the software points towards https://cryptoknight.cc/ and references the $ per KH per seconds per day metric. Because of the different algorithms each coin uses, its not possible to compare without the benchmarked rates.
##### Compatible Coins:
All Coins listed on https://cryptoknight.cc/ are technically compatible.

#### Mining:
Coin profitability is checked every 2 minutes the results are echoed.
##### Switching Criteria:
A switch will occur under 2 criteria:
- A more profitable coin is available, and the current coin has been mined for over 60 mins.
  - This is to insure mining continues without stopping and starting frequently.
  - And to try to get enough shares to warrant a pay-out.
- A more profitable coin is available and is > 20% more profitable than the current coin.

##### Cast-xmr Share monitoring:
If a share is not submitted in 15 minutes, the software benchmarks and starts again.
