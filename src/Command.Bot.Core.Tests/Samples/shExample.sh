#!sh
RIGHT_NOW=$(date +"%x %r %Z")
echo hello
echo i am a sh file running at $RIGHT_NOW
echo Argument was $1

curl --fail http://sssss.ss/test.com