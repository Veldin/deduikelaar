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
        return $this->hasManyThrough(Story::class, StoryFeedback::class, 'feedbackId', 'id', 'id', 'storyId');
    }
    public function storyFeedback(){
        return $this->hasMany(StoryFeedback::class, 'feedbackId', 'id');
    }

    public function getJson($story = null, $storyFeedback = null){
        $r = [
            "feedbackId" => $this->id,
            "answer" => $this->feedback
        ];

        if($story){
            if($storyFeedback){

                $r['count'] = 0;
                foreach ($storyFeedback as $val){
                    if( ( isset($val->storyId) &&
                        $val->storyId != $story->id ) ||
                        $val->feedbackId != $this->id ) continue;
                        $r['count'] = intval($val->count);
                        break;
                }
            }else{
                $r['count'] = StoryFeedback::where([
                    ['storyId','=',$story->id],
                    ['feedbackId','=',$this->id],
                ])->count();
            }
        }


        return $r;

    }

}