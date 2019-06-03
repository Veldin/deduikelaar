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

        if(Feedback::where('question', 'Wat was je gevoel hierbij?')->count()){

            $feedbackItems = FeedbackItem::get();


            foreach ($feedbackItems as $feedbackItem){
                $feedbackItems[] = $feedbackItem;
            }

        }else{

            // Wat was je gevoel hierbij?
            $feedback = Feedback::create([
                'question' => 'Wat was je gevoel hierbij?',
                'extraInfo' => '',
                'feedbackType' => 'emoticon',
                'oneWord' => 'Gevoel'
            ]);
            $feedbackItems[] = FeedbackItem::create([
                'feedback' => '\u1F603',
                'feedbackId' => $feedback->id
            ]);

            $feedbackItems[] = FeedbackItem::create([
                'feedback' => '\u1F620',
                'feedbackId' => $feedback->id
            ]);

            $feedbackItems[] = FeedbackItem::create([
                'feedback' => '\u1F622',
                'feedbackId' => $feedback->id
            ]);

            // Was het goed leesbaar?
            $feedback = Feedback::create([
                'question' => 'Was het goed leesbaar?',
                'extraInfo' => '',
                'feedbackType' => 'ja/nee',
                'oneWord' => 'Leesbaar'
            ]);

            $feedbackItems[] = FeedbackItem::create([
                'feedback' => 'Ja',
                'feedbackId' => $feedback->id
            ]);

            $feedbackItems[] = FeedbackItem::create([
                'feedback' => 'Nee',
                'feedbackId' => $feedback->id
            ]);

            // Hoe interessant vond je het?
            $feedback = Feedback::create([
                'question' => 'Hoe interessant vond je het?',
                'extraInfo' => '',
                'feedbackType' => 'tekst',
                'oneWord' => 'Interessant'
            ]);

            $feedbackItems[] = FeedbackItem::create([
                'feedback' => 'Eg interessant',
                'feedbackId' => $feedback->id
            ]);

            $feedbackItems[] = FeedbackItem::create([
                'feedback' => 'Interessant',
                'feedbackId' => $feedback->id
            ]);

            $feedbackItems[] = FeedbackItem::create([
                'feedback' => 'Minder interessant',
                'feedbackId' => $feedback->id
            ]);

            // Hoe goed heb jij het begrepen?
            $feedback = Feedback::create([
                'question' => 'Hoe goed heb jij het begrepen?',
                'extraInfo' => '',
                'feedbackType' => 'tekst',
                'oneWord' => 'Duidelijkheid'
            ]);

            $feedbackItems[] = FeedbackItem::create([
                'feedback' => 'Grotendeels begrepen',
                'feedbackId' => $feedback->id
            ]);

            $feedbackItems[] = FeedbackItem::create([
                'feedback' => 'Begrepen',
                'feedbackId' => $feedback->id
            ]);

            $feedbackItems[] = FeedbackItem::create([
                'feedback' => 'Niet begrepen',
                'feedbackId' => $feedback->id
            ]);
        }


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
