<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class StoryItem extends Model
{
    /**
     * The table associated with the model.
     *
     * @var string
     */
    protected $table = 'storyItem';

    protected $fillable = [
        'text',
        'storyId'
    ];

    public function story() {
        return $this->belongsToMany(Story::class,'storyId','id');
    }
    public function file(){
        return $this->hasOne(File::class);
    }
}