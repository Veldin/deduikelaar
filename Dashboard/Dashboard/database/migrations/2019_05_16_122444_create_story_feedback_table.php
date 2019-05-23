<?php

use Illuminate\Support\Facades\Schema;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Database\Migrations\Migration;

class CreateStoryFeedbackTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('storyFeedback', function (Blueprint $table) {

            $table->integer('storyId')->unsigned()->nullable();
            $table->foreign('storyId')->references('id')
                ->on('story')->onDelete('cascade');

            $table->integer('feedbackId')->unsigned()->nullable();
            $table->foreign('feedbackId')->references('id')
                ->on('feedbackItem')->onDelete('cascade');

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
        Schema::dropIfExists('storyFeedback');
    }
}
