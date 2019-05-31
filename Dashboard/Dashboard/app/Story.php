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

    public function allFeedback(){

        $data = [];
        foreach ($this->feedback as $feedback){
            $question = $feedback->question;

             if(!isset($data[$question->id])){
                 $data[$question->id] = [
                     'id' => $question->id,
                     'question' => $question->question,
                     'extraInfo' => $question->extraInfo,
                     'feedbackType' => $question->feedbackType,
                     'feedback' => []
                 ];
             }
             if(!isset($data[$question->id]['feedback'][$feedback->id])){
                 $data[$question->id]['feedback'][$feedback->id] = [
                     'answer' => $feedback->feedback,
                     'count' => 1
                 ];
             }else{
                 $data[$question->id]['feedback'][$feedback->id]['count']++;
             }
        }

        foreach ($data as $k => $v){
            $fb = $v['feedback'];
            usort($fb, function($a, $b){
                if($a['count'] == $b['count']) return 0;
                return ($a['count'] > $b['count']) ? -1 : 1;
            });
            $data[$k]['feedback'] = $fb;
        }
        usort($data, function(){
            return 0;
        });

        return $data;
    }
}