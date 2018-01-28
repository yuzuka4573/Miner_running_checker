# Miner_running_checker
To check Miner mining process in MPOS mining pool with MPOS APIs.  
  
## More info   
This is Windows console application.  
You can see Miner States, HashRate, ShareRate and Amplification of Confirmed Coin in MPOS mining pool. 
Runnning program, set timer interval which get information from mining pool.  
  
## Install
I will write it soon (or clone and build this project!!)  
  
## Setup
This program need "profile.txt" in same directory.
In profile.txt, write like it (4line 1sets!!)

-------------------------------------------------------------------------------------------------------------------------------  
Your Miner Name (Ex. My mining Rig001)
Coin Type(Ex. BTC)
MPOS API URLs[getuserstates] (Ex. https://[your mining pool].com/index.php?page=api&action=getuserstatus&api_key=[your API key])
MPOS API URLs[getuserbalance] (Ex. https://[your mining pool].com/index.php?page=api&action=getuserbalance&api_key=[your API key])
-------------------------------------------------------------------------------------------------------------------------------  

In MPOS API URLs, You can find API key and URLs in My Account>Edit Account>Account Details>API Key (meybe...?)  

## How to use  
1. Set profile.txt
2. Running Program
3. set timer interval

