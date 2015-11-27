#[CLK.Promises] 跨平台的非同步Promise模型#

Promises library for .NET, Java, and Objective-C


##問題情景##

跨平台APP的開發，可以透過跨平台開發流程，來整合SA、SD重覆的工作項目，產出共用的SA、SD文件用以減少分析設計人員的負擔。但是在遇到功能需求，需要被設計為非同步的開發情景時，因為各平台支援的非同步模型都不盡相同，很難設計為相同的SD物件模型，所以就被迫建立出多份相同但有些微差異的SD文件。這些SD文件裡些微的差異，隨著系統的成長會呈等比的繼續擴大，最終變化為完全不同的多份SD文件，增加後續維護的難度與成本。

 - 參考資料 : [[Architecture Design] 跨平台開發流程](https://dotblogs.com.tw/clark/2015/03/27/150861)

 - 問題情景 :
 
	 - ![問題情景01](https://raw.githubusercontent.com/Clark159/clark159.github.io/master/_posts/%5BCLK.Promises%5D/%5BCLK.Promises%5D%20%E8%B7%A8%E5%B9%B3%E5%8F%B0%E7%9A%84%E9%9D%9E%E5%90%8C%E6%AD%A5Promise%E6%A8%A1%E5%9E%8B/%E5%95%8F%E9%A1%8C%E6%83%85%E6%99%AF01.png)


##解決方案##

問題情景的根本原因，在於沒有一個跨平台的非同步模型來提供系統設計人員使用。本篇文章介紹一個橫跨Windows(C#)、Android(Java)、iOS(Objective-C)三個平台的非同步Promise模型實作，讓系統設計人員在遇到跨平台APP設計的時候，能夠有個統一的非同步模型可以選擇，用以弭平不同平台間系統設計時的差異。主要為自己留個紀錄，也希望能幫助到有需要的開發人員。

- 開源位址 : [CLK.Promises](https://github.com/Clark159/CLK.Promises)

- 解決方案 :
 
	- ![解決方案01](https://raw.githubusercontent.com/Clark159/clark159.github.io/master/_posts/%5BCLK.Promises%5D/%5BCLK.Promises%5D%20%E8%B7%A8%E5%B9%B3%E5%8F%B0%E7%9A%84%E9%9D%9E%E5%90%8C%E6%AD%A5Promise%E6%A8%A1%E5%9E%8B/%E8%A7%A3%E6%B1%BA%E6%96%B9%E6%A1%8801.png)


##使用說明##

- Windows平台使用說明 (C#)

- Android平台使用說明 (Java)

- iOS平台使用說明 (Objective-C)


##參考資料##

- [JavaScript Promise迷你書（中文版）](http://liubin.github.io/promises-book/)

- [Promises/A+](https://promisesaplus.com/)

