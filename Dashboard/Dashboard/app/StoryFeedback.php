<?php

namespace App;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Support\Facades\DB;

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

    public static function getCount($story = null){
        if($story){
            return DB::table('storyFeedback')
                ->select([DB::raw('COUNT(*) as count'), 'feedbackId'])
                ->where('storyId', $story->id)
                ->groupBy(['feedbackId'])
                ->get();
        }else{
            return DB::table('storyFeedback')
                ->select([DB::raw('COUNT(*) as count'), 'storyId', 'feedbackId'])
                ->groupBy(['storyId','feedbackId'])
                ->get();
        }
    }

}