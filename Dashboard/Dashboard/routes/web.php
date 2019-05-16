<?php

/*
|--------------------------------------------------------------------------
| Web Routes
|--------------------------------------------------------------------------
|
| Here is where you can register web routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| contains the "web" middleware group. Now create something great!
|
*/


use Illuminate\Support\Facades\Route;

Route::get('/', function () {
    return view('welcome');
});


Route::get('/test', function(){
    $story = \App\Story::create([
            'title' => "test".count(\App\Story::get()),
            'icon' => "testIcon".count(\App\Story::get()),
            'description' => "Geen",
        ]);
    $feedback = \App\Feedback::create([
        'question' => "feedback".count(\App\Feedback::get()),
    ]);
    $feebackItem = \App\FeedbackItem::create([
        'feedback' => "feedback1 for ".count(\App\Feedback::get()),
        'feedbackId' => $feedback->id
    ]);

    $storyFeedback = \App\StoryFeedback::create([
        'storyId' => $story->id,
        'feedbackId' => $feebackItem->id
    ]);

    $feedbacks = \App\Feedback::with('feedbackItems', 'feedbackItems.stories')->get();

    foreach ($feedbacks as $feedback){
//        dd($feedback, $feedback->feedbackItems(), $feedback->feedbackItems);

        echo "Question: " . $feedback->question . "<br />";
        foreach ($feedback->feedbackItems as $feedbackItem){
            echo "Answer: " . $feedbackItem->feedback . "<br />";
            foreach ($feedbackItem->stories as $story){
                echo "Story title: " . $story->title . "<br />";
            }
        }
        echo "<br />";
        echo "<hr>";
        echo "<br />";
    }

    $stories = \App\Story::with('feedback')->get();
    foreach ($stories as $story){

        echo "Story title: " . $story->title . "<br />";

        foreach ($story->feedback as $feedbackItem){
            echo "Answer: " . $feedbackItem->feedback . "<br />";
        }

        echo "<br />";
        echo "<hr>";
        echo "<br />";
    }

    return "";
});

