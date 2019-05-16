<?php

use Illuminate\Support\Facades\Schema;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Database\Migrations\Migration;

class CreateFileTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('file', function (Blueprint $table) {
            $table->increments('id');
            $table->string('fileName');
            $table->string('realName');
            $table->string('fileType');
            $table->string('extension');
            $table->string('path');

            $table->integer('storyItemId')->unsigned();
            $table->foreign('storyItemId')
                ->references('id')->on('storyItem')
                ->onDelete('cascade');
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('file');
    }
}
