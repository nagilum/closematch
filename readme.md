# CloseMatch

Cmd app to search multiple files for multiple instances of text close to each other.

Usage:

```cm.exe -s text1 -s text2 [-c 1] [-l 2] [-p path] [-e *.cs] [-r] [-v] [-x]```

* ```-s``` Add texts to search for. Require at least one.
* ```-c``` Make sure each string match is no further than n chars away. Defaults to 0.
* ```-l``` Make sure each string match is no further than n lines away. Defaults to 5.
* ```-p``` Set path to search in. Defaults to current working directory.
* ```-e``` Set extensions to search for. Defaults to *.*
* ```-r``` Do a recursive scan. Defaults to off.
* ```-v``` Output not only the files with hits and the lines, but also the actual text. Defaults to off.
* ```-x``` Shows errors, if any. Defatults to off.
