<?php

/* @var $factory \Illuminate\Database\Eloquent\Factory */

use App\File;
use Faker\Generator as Faker;
use Illuminate\Support\Facades\Storage;

$factory->define(File::class, function (Faker $faker) {
    $file = $faker->dayOfMonth . "-" . $faker->month . "-" . $faker->year;
    $ext = $faker->fileExtension;
    $filename = $file."_".rand(11111111,99999999).".".$ext;

    Storage::put("uploads/".$filename, $faker->realText());

    return [
        'fileName' => $filename,
        'realName' => $file.".".$ext,
        'fileType' => $ext,
        'extension' => $ext,
        'path' => 'uploads'
    ];
});
