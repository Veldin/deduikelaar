<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class File extends Model
{
    /**
     * The table associated with the model.
     *
     * @var string
     */
    protected $table = 'file';


    protected $fillable = [
        'fileName',
        'realName',
        'fileType',
        'extension',
        'path',
        'storyItemId'
    ];

    public function storyItem() {
        return $this->belongsTo(StoryItem::class,'storyItemId','id');
    }
}