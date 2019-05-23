<?php

use App\Feedback;
use App\FeedbackItem;
use App\File;
use App\Story;
use App\StoryFeedback;
use App\StoryItem;
use Illuminate\Database\Seeder;

class DatabaseSeeder extends Seeder
{
    /**
     * Seed the application's database.
     *
     * @return void
     */
    public function run()
    {
        $feedbackItems = [];
        // $this->call(UsersTableSeeder::class);
        $feedback = Feedback::create([
            'question' => 'Wat was je gevoel hierbij?',
            'extraInfo' => '',
            'feedbackType' => 'emoticon',
        ]);
        $feedbackItems[] = FeedbackItem::create([
            'feedback' => '\u1F603',
            'feedbackId' => $feedback->id
        ]);

        $feedbackItems[] = FeedbackItem::create([
            'feedback' => '\u1F634',
            'feedbackId' => $feedback->id
        ]);

        $feedbackItems[] = FeedbackItem::create([
            'feedback' => '\u1F92F',
            'feedbackId' => $feedback->id
        ]);

        $feedback = Feedback::create([
            'question' => 'Was het duidelijk voor jou?',
            'extraInfo' => '',
            'feedbackType' => 'ja/nee',
        ]);

        $feedbackItems[] = FeedbackItem::create([
            'feedback' => 'Ja',
            'feedbackId' => $feedback->id
        ]);

        $feedbackItems[] = FeedbackItem::create([
            'feedback' => 'Nee',
            'feedbackId' => $feedback->id
        ]);



        factory(Story::class, 20)->create()->each(function (Story $story) use ($feedbackItems) {

            for($i=0; $i<rand(0,1000);$i++){
                //
                StoryFeedback::create([
                    'storyId' => $story->id,
                    'feedbackId' => $feedbackItems[mt_rand(0,count($feedbackItems)-1)]->id
                ]);
            }

            for($i=0;$i<rand(1,5);$i++){
                $storyItem = factory(StoryItem::class)->create([
                    'storyId' => $story->id
                ]);
                if(rand(0,5) < 2){
                    factory(File::class)->create([
                        'storyItemId' => $storyItem->id
                    ]);
                }

            }

        });


//            ->create()->each(function (Story $story) use ($feedbackItems) {
//            $story->storyItems()->save($feedbackItems[mt_rand(0,count($feedbackItems))]);
//        });



    }
}
