Last Update of this Readme: 02.01.2019

=== ENGLISH ===

**** How to setup your Multi-User-VR-game ****

Pre-requisites:
- Get VRTK here: https://github.com/thestonefox/VRTK
- Install SteamVR (Version 1.2.3! NOT the latest one.): https://github.com/ValveSoftware/steamvr_unity_plugin/releases/download/1.2.3/SteamVR.Plugin.unitypackage
- Get Photon PUN 2.0: https://assetstore.unity.com/packages/tools/network/pun-2-free-119922

Setup:
1. Sign up or Login at PhotonEngine for free to get your AppId (necessary for setting up online games): https://www.photonengine.com/
2. Enter your AppId into the field of the PUN Setup Wizard (in Unity: Window -> Photon Unity Networking -> PUN Wizard)
3. Open MUVRTK -> Examples -> "01 - Lobby" for some example implementations of MUVRTK
4. If your game shall not have a Lobby, you can check out the Common Room Scene straight away.


IMPORTANT NOTES / TROUBLESHOOTING:
- do NOT change the VRTK SDK Setup Settings! If changed, they might cause connection problems with Photon.
- if you have trouble connecting, try changing the wifi connection you're using, e.g. eduroam usually doesn't work.