<?php

use Illuminate\Support\Facades\Schema;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Database\Migrations\Migration;

class CreateStoryItemTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('storyItem', function (Blueprint $table) {
            $table->increments('id')->autoIncrement();
            $table->longText('text')->nullable();
            $table->integer('storyId')->unsigned();
            $table->foreign('storyId')
                ->references('id')->on('story')
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
        Schema::dropIfExists('storyItem');
    }
}
