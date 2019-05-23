<?php

/* @var $factory \Illuminate\Database\Eloquent\Factory */

use App\Story;
use Faker\Generator as Faker;

$factory->define(Story::class, function (Faker $faker) {
    $icons = ['candle', 'letter', 'paper', 'speaker', 'envaloppe', 'playButton'];
    return [
        'title' => $faker->realText(40),
        'description' => $faker->realText(),
        'active' => $faker->boolean(80),
        'icon' => $icons[$faker->numberBetween(0, 5)]
    ];
});
