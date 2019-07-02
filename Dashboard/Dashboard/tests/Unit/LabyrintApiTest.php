<?php

namespace Tests\Unit;

use App\File;
use App\Http\Controllers\Api\StoryApiController;
use App\Story;
use finfo;
use Illuminate\Http\UploadedFile;
use Illuminate\Support\Facades\Storage;
use Tests\TestCase;
use Illuminate\Foundation\Testing\WithFaker;
use Illuminate\Foundation\Testing\RefreshDatabase;

class LabyrintApiTest extends TestCase
{

    public function testReturnFeedback(){

//[
//    {
//        storyId: 1,
//        answerId: 2,
//    },
//    {
//        storyId: 3,
//        answerId: 1,
//    },
//]

        $storyOne = Story::with('feedback')->find(1);
        $storyTwo = Story::with('feedback')->find(3);

//
//        $firstCount = 0;
//        $secondCount = 0;
//        foreach ($story->feedback as $feedback){
//            if($feedback->id == 2){
//                $firstCount++;
//            }
//        }
//        $story = Story::with('feedback')->find(3);
//        $secondCount = 0;
//        foreach ($story->feedback as $feedback){
//            if($feedback->id == 1){
//                $secondCount++;
//            }
//        }

        $data = [
            [
                'storyId' => 1,
                'answerId' => 2
            ],
            [
                'storyId' => 3,
                'answerId' => 1
            ],
        ];
        // Check success
        $response = $this->json('POST', '/api/v1/feedback', $data);
//        var_dump($response->json());
        $this->assertEquals('success', $response->json()['response']);


        $storyOneSecond = Story::with('feedback')->find(1);
        $storyTwoSecond = Story::with('feedback')->find(3);

        $this->assertEquals($storyOne->feedbackCount(2)+1, $storyOneSecond->feedbackCount(2));
        $this->assertEquals($storyTwo->feedbackCount(1)+1, $storyTwoSecond->feedbackCount(1));



    }


    public function databaseSeed(){
        $dbs = new \DatabaseSeeder();
        $dbs->run();
    }

//    private function getFeedbackCount($storyId, $answerId){
//        $story = Story::with('feedback')->find($storyId);
//        $c = 0;
//        foreach ($story->feedback as $feedback){
//            if($feedback->id == $answerId){
//                $c++;
//            }
//        }
//        return $c;
//    }
}

