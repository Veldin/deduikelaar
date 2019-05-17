<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class FeedbackItem extends Model
{
    /**
     * The table associated with the model.
     *
     * @var string
     */
    protected $table = 'feedbackItem';

    protected $fillable = [
        'feedback',
        'feedbackId',
    ];

    public function question() {
        return $this->belongsTo(Feedback::class,'feedbackId','id');
    }

    public function stories()
    {
//        return $this->hasMany(StoryFeedback::class, 'feedbackId', 'id');
        return $this->hasManyThrough(Story::class, StoryFeedback::class, 'feedbackId', 'id', 'id', 'storyId');

//        class Country extends Model
//        {
//            public function posts()
//            {
//                return $this->hasManyThrough(
//                    'App\Post',
//                    'App\User',
//                    'country_id', // Foreign key on users table...
//                    'user_id', // Foreign key on posts table...
//                    'id', // Local key on countries table...
//                    'id' // Local key on users table...
//                );
//            }
//        }
    }
}