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

    // TODO: PDF omzetten wanneer tijd over
    public static $allowedExtensions = ['docx','jpeg','jpg','png','gif','bmp','avi','mp4','mpeg'];

    public static $allowedImageFiles = ['jpeg','jpg','png','gif','bmp'];
    public static $allowedTextFiles = ['docx'];
    public static $allowedVideoFiles = ['avi','mp4','mpeg'];

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


    /**
     * Convert a docx file to text
     * @return bool|string|null
     */
    public function convertDocxFile(){

        // Check if it is a docx file
        if(!in_array($this->extension, self::$allowedTextFiles)) return null;
        $content = '';

        // Open file as a zip file
        $zip = zip_open(storage_path($this->path.$this->fileName));

        // Check if it is an zip file
        if (!$zip || is_numeric($zip)) return null;
        $media = [];
        $relations = "";

        // Loop through files
        while ($zip_entry = zip_read($zip)) {

            if (zip_entry_open($zip, $zip_entry) == FALSE) continue;

            // Set relations
            if(substr(zip_entry_name($zip_entry), 0, 11) == "word/_rels/"){
                $relations = zip_entry_read($zip_entry, zip_entry_filesize($zip_entry));
            }

            // Set media
            if(substr(zip_entry_name($zip_entry), 0,11) == "word/media/"){

                $media[substr(zip_entry_name($zip_entry), 5)] = [
                    'size' => zip_entry_filesize($zip_entry),
                    'data' => base64_encode(zip_entry_read($zip_entry, zip_entry_filesize($zip_entry)))
                ];
            }

            // Ignore all other files, except for the actual document
            if (zip_entry_name($zip_entry) != "word/document.xml") continue;

            // Set the content
            $content .= zip_entry_read($zip_entry, zip_entry_filesize($zip_entry));

            zip_entry_close($zip_entry);
        }// end while

        zip_close($zip);


        // Read relations as XML
        $xmlRel = simplexml_load_string($relations);

        // Change media array to key -> value where key is the id from relations file
        foreach ($xmlRel->Relationship as $relation) {
            $attributes = $relation->attributes();
            if(strpos((string) $attributes->Type, 'image') !== false){
                $media[(string)$attributes->Id] = $media[(string)$attributes->Target];
                unset($media[(string)$attributes->Target]);
            }
        }

        // Read document as XML
        $xml = simplexml_load_string($content,null, 0, 'w', true);

        // find all namespaces
        preg_match_all('/xmlns:(.*?)="(.*?)"/i', $content, $matches);

        // Add all the namespaces to the xml reader
        $namespaces = [];
        if(count($matches) > 2){
            foreach ($matches[1] as $i => $v){
                if(isset($matches[2][$i])){
                    $xml->registerXPathNamespace($v, $matches[2][$i]);
                    $namespaces[$v] = true;
                }
            }
        }

        // Document body
        $body = $xml->body;

        $content = "";
        $mediaIndex = 0;
        // Loop through each tag that is directly in the body tag
        foreach($body[0] as $key => $value){

            // Open html paragraph
            $content .= "<p>";
            if($key == "p"){

                // Set text
                foreach ($value->r as $kkey => $vvalue) {
                    $content .= (string)$vvalue->t;
                }

                // Check if there are images
                $drawing = $this->findDrawing($value);
                if($drawing){
                    // Find image id
                    $search = $drawing->xpath("//*[local-name()='blip']");
                    if(isset($search[$mediaIndex])){
                        if(isset($search[$mediaIndex]->attributes('r', true)[0])){
                            $id = (string) $search[$mediaIndex]->attributes('r', true)[0];
                            if(isset($media[$id])){
                                // image id found
                                // Add image tag with base64 source
                                $content .= "<img src='data:image/png;base64, ".$media[$id]['data']."'/>";
                            }
                            $mediaIndex++;
                        }
                    }
                }
            }

            // End html paragraph
            $content .= "</p>";
        }

        return $content;

    }

    public function convertImageFile(){

        // Check if it is an image file
        if(!in_array($this->extension, self::$allowedImageFiles)) return null;

        $path = storage_path($this->path.$this->fileName);
        $type = pathinfo($path, PATHINFO_EXTENSION);
        $data = file_get_contents($path);
        $base64 = 'data:image/' . $type . ';base64,' . base64_encode($data);

        return "<img src='".$base64."' alt='".$this->realName."'>";
    }

    public function convertVideoFile(){

        return "";
    }

    /**
     * Find drawing tag for image
     * @param $v \SimpleXMLElement
     * @return \SimpleXMLElement|null
     */
    private function findDrawing($v){

        // Find drawing tag
        foreach ($v->r as $l => $w) {
            if(isset($w->drawing)){
                return $w->drawing;
            }
            if(count($w) > 0){
                $d = $this->findDrawing($w);
                if($d != null) return $d;
            }
        }
        return null;
    }



}