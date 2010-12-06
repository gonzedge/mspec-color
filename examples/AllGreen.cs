using System;
using Machine.Specifications;

[Subject("Dogs")]
public class dogs_doin_stuff {
	It should_do_green_stuff =()=> "Hi".ShouldEqual("Hi");
}

[Subject("Cats")]
public class cats_doin_stuff {
	It should_do_green_stuff =()=> "Hi".ShouldEqual("Hi");
	It should_do_more_green_stuff =()=> "Hi".ShouldEqual("Hi");
}
