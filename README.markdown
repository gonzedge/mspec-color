MSpec Color
===========

I recently started using [MSpec][], which is an awesome framework for doing BDD in .NET

It's REALLY annoying, however, that mspec.exe doesn't color its output.  It's just silly!

So I made this quick and dirty script to wrap mspec.exe and print out colors.

[Download mspec.exe][download]

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

Any arguments you pass to mspec-color.exe get passed straight to mspec.exe.  So if can alias 'mspec' 
on your system to 'mspec-color' and commands like `mspec /?` will still run fine.

By default, `mspec` is run.  If you need to set the path to the mspec executable to be run, you can set the `MSPEC_PATH` environment variable.  If it's found, it will run that instead of simply assuming that `mspec` is in your PATH and will run fine.

That's it!
----------

Nothing too crazy, just a tiny little script.

License
-------

MSpec Color is released under the MIT license.

[mspec]:    https://github.com/machine/machine.specifications
[download]: https://github.com/remi/mspec-color/raw/master/mspec-color.exe
[with]:     https://github.com/remi/mspec-color/raw/master/examples/with-color.png
[without]:  https://github.com/remi/mspec-color/raw/master/examples/without-color.png
