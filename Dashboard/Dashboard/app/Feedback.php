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

    public function getJson($story = null){
        $feedbackItems = [];

        /** @var FeedbackItem $feedbackItem */
        foreach ($this->feedbackItems as $feedbackItem){
            $feedbackItems[] = $feedbackItem->getJson($story);
        }

        $r = [
            'feedbackId' => $this->id,
            'question' => $this->question,
            'extraInfo' => $this->extraInfo,
            'feedbackType' => $this->feedbackType,
            'oneWord' => $this->oneWord,
            'feedback' => array_values($feedbackItems)
        ];

        if($story){
            $r['count'] = 0;
            foreach ($feedbackItems as $feedbackItem){
                $r['count'] += $feedbackItem['count'];
            }
        }

        return $r;
    }
}