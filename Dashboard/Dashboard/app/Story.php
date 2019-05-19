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
        return $this->hasMany(StoryItem::class);
    }


    public function feedback()
    {
//        return $this->hasManyThrough(Story::class, StoryFeedback::class, 'feedbackId', 'id', 'id', 'storyId');
//        return $this->belongsToMany(Feed::class, 'storyFeedback');
//        return $this->hasMany(StoryFeedback::class, 'storyId');
        return $this->hasManyThrough(FeedbackItem::class, StoryFeedback::class, 'storyId', 'id', 'id', 'feedbackId');
    }
}