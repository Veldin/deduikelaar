<?php

/* @var $factory \Illuminate\Database\Eloquent\Factory */

use App\StoryItem;
use Faker\Generator as Faker;

$factory->define(StoryItem::class, function (Faker $faker) {
    return [
        'text' => $faker->realText(40)
    ];
});
