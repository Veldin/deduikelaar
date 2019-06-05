<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class Feedback extends Model
{
    /**
     * The table associated with the model.
     *
     * @var string
     */
    protected $table = 'feedback';


    protected $fillable = ['question', 'extraInfo', 'feedbackType', 'oneWord'];


    public function feedbackItems(){
        return $this->hasMany(FeedbackItem::class, 'feedbackId', '');
    }

    public function getJson($story = null, $storyFeedback = null){
        $feedbackItems = [];

        /** @var FeedbackItem $feedbackItem */
        foreach ($this->feedbackItems as $feedbackItem){
            $feedbackItems[$feedbackItem->id] = $feedbackItem->getJson($story, $storyFeedback);
        }

        $r = [
            'feedbackId' => $this->id,
            'question' => $this->question,
            'extraInfo' => $this->extraInfo,
            'feedbackType' => $this->feedbackType,
            'oneWord' => $this->oneWord,
            'feedback' => $feedbackItems
        ];

        if($story){
            $r['count'] = 0;
            foreach ($feedbackItems as $feedbackItem){
                if(isset($feedbackItem['count'])){
                    $r['count'] += $feedbackItem['count'];
                }
            }
        }

        return $r;
    }
}