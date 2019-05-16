<?php

use Illuminate\Support\Facades\Schema;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Database\Migrations\Migration;

class CreateFeedbackItemTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('feedbackItem', function (Blueprint $table) {
            $table->bigIncrements('id');
            $table->string('feedback');

            $table->integer('feedbackId')->unsigned();
            $table->foreign('feedbackId')
                ->references('id')->on('feedback')
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
        Schema::dropIfExists('feedbackItem');
    }
}
