# cTrader-algos
C# bots / indicators

https://ctrader.com/algos/indicators/show/1788

The Inverse Fisher Transform version of RSI indicator created by John Ehlers. 
https://www.mesasoftware.com/papers/TheInverseFisherTransform.pdf

The purpose of this indicator is to help with determining the turn points on the market and improve timing decisions. 
Signals are more clear and unequivocal thanks to smoothing function and logarithmic equation (this method can be applied to most of the oscillator-type indicators).

The computation adapted by myself onto the C# language for cTrader platform.

BUY when the indicator crosses over -0.5 or crosses over 0.5 if it has not previously crossed over -0.5.
SELL when the indicator crosses under 0.5 or crosses under -0.5 if it has not previously crossed under 0.5.

![image](https://user-images.githubusercontent.com/88622607/131832253-c32f42ac-901d-4ba3-8021-a77ee4fd1d8b.png)
