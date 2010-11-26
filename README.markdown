NUnit Color Console
===================

I run nunit-console.exe a lot.  The GUI is nice and all but the console is wicked fast 
and easy to integrate into my text editor, run via BATCH/BASH files, etc.

It's REALLY annoying, however, that nunit-console.exe doesn't color its output.  It's just silly!

So I made this quick and dirty script to wrap nunit-console.exe and print out colors.

### With Colors

![Screenshot With Colors][with]

### Without Colors

![Screenshot Without Colors][without]

Colors
------

This colors the summary line red if anything failed, green is everything passed, or yellow if no tests actually ran at all.

This colors the "Test Case Failures" section red.

This colors the "Tests not run" section yellow.

Running It
----------

Any arguments you pass to nunit-color-console.exe get passed straight to nunit-console.exe.  So if can alias 'nunit-console' 
on your system to 'nunit-color-console' and commands like `nunit-console /?` will still run fine.

That's it!
----------

Nothing too crazy, just a tiny little script.

License
-------

NUnit Color Console is released under the MIT license.

[with]:    http://github.com/remi/nunit-color-console/raw/master/examples/with-color.png
[without]: http://github.com/remi/nunit-color-console/raw/master/examples/without-color.png
