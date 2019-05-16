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


    protected $fillable = ['question', 'extraInfo', 'feedbackType'];


    public function feedbackItems(){
        return $this->hasMany(FeedbackItem::class, 'feedbackId', '');
    }

}