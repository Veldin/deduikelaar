<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class StoryFeedback extends Model
{
    /**
     * The table associated with the model.
     *
     * @var string
     */
    protected $table = 'storyFeedback';


    protected $fillable = [
        'storyId',
        'feedbackId'
    ];

    public function stories() {
        return $this->belongsTo(Story::class,'id','storyId');
    }
    public function feedbackItems() {
        return $this->belongsTo(FeedbackItem::class,'id','feedbackId');
    }
}