There are a few paid assets that need to be put in :

The way to do this is to
1. Make a PaidAssets folder in the Assets folder
2. Add the following assets to this folder
LED Camping Lantern Torch PBR : https://assetstore.unity.com/packages/3d/props/electronics/led-camping-lantern-torch-pbr-106936
Modular Horror Corridor : https://assetstore.unity.com/packages/3d/environments/modular-horror-corridor-264581
Radio : https://assetstore.unity.com/packages/3d/props/radio-230712

The Radio is free, but was made by the same people as the Modular Horror Corridor, so got bundled into the same folder.

This uses the Meta all in one VR SDK

If you look at the class UdpListener, you can also see that it listens for UDP packets on the 2910 port.
Technically, the HeartRateMonitor just listens for UDP updates, and then parses out anything that is a pure int value. So you can use any UDP into message to drive the lamp.

I'm using this script with this :
https://github.com/sharramon/Heartrate_ESP32_UDP
