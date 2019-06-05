<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class Story extends Model
{
    /**
     * The table associated with the model.
     *
     * @var string
     */
    protected $table = 'story';


    protected $fillable = ['title', 'icon', 'description', 'active'];


    public function storyItems(){
        return $this->hasMany(StoryItem::class, 'storyId', 'id');
    }


    public function feedback()
    {
        return $this->hasManyThrough(FeedbackItem::class, StoryFeedback::class, 'storyId', 'id', 'id', 'feedbackId');
    }

    public function feedbackCount($answerId){
        $c = 0;
        foreach ($this->feedback as $feedback){
            if($feedback->id == $answerId) $c++;
        }
        return $c;
    }

    public function storyFeedback(){
        return $this->hasMany(StoryFeedback::class, 'storyId', 'id');
    }
    public function allFeedback($feedbacks = null, $storyFeedback = null){

        $feedbackJson = [];
        if(!$feedbacks) $feedbacks = Feedback::with('feedbackItems')->get();

        /** @var Feedback $feedback */
        foreach ($feedbacks as $feedback) {
            $feedbackJson[$feedback->id] = $feedback->getJson($this, $storyFeedback);
        }
        return array_values($feedbackJson);
    }
}