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