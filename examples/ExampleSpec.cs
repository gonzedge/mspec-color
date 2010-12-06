using System;
using Machine.Specifications;

[Subject("Dogs")]
public class dogs_doin_stuff {
	It should_do_green_stuff =()=> "Hi".ShouldEqual("Hi");

	It should_do_red_stuff =()=> "Hi".ShouldEqual("Definitely not Hi");

	It should_do_yellow_stuff;
}
