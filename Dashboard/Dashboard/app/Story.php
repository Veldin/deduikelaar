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
        return $this->hasManyThrough(FeedbaphpckItem::class, StoryFeedback::class, 'storyId', 'id', 'id', 'feedbackId');
    }

}