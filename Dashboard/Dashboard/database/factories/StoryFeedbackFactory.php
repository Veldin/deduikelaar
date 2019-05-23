<?php

/* @var $factory \Illuminate\Database\Eloquent\Factory */

use App\StoryFeedback;
use Faker\Generator as Faker;

$factory->define(StoryFeedback::class, function (Faker $faker) {

    return [
        'feedbackId' => $faker->numberBetween(0, 6)
    ];
});
