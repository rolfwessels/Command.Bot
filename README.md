# Command.Bot 


# Installing Command bot

* Add "bots" to your slack installation. (on https://slack.com/apps search for bots and add it. Give the bot the username that you desire)
* Copy the API Token 
* Download the bot from https://github.com/rolfwessels/Command.Bot/releases
* Extract bot into working folder.
* Copy the Command.Bot.exe.sample.config to Command.Bot.exe.config file, update the BotKey setting and the AllowedUser settings. (Note allow user can be multiple users, that are comma seperated)
* For testing you can run "Command.Bot.exe service run"
* Once that is working you can add command bot as a service by running  "Command.Bot.exe service install" as an *administator*


# Using command bot.

* First type 'help' as a dm to the bot that you have just added.
* Add files to /script folder to be able to execute them
* type the file name of the script that you want to execute to run it.

# Developers 

* Build release for github
 update the version`
 go deploy -properties @{'buildConfiguration'='Release'}
