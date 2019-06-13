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
    public static $allowedExtensions = ['docx','jpeg','jpg','png','gif','bmp','avi','mp4','mpeg', 'webm', 'mp3','wav'];

    public static $imageFileExtensions = ['jpeg','jpg','png','gif','bmp'];
    public static $videoFilesExtensions = ['avi','mp4','mpeg', 'webm'];
    public static $audioFilesExtensions = ['mp3','wav'];

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

    public function getFileAsText(){
        $text = "";
        // Image files
        if(in_array($this->extension, self::$imageFileExtensions)){
            $text = $this->convertImageFile();
        }
        // Word files
        if($this->extension == 'docx'){
            // Set story item text if the file is an docx file
            $text = $this->convertDocxFile();
        }
        // PDF files
        if($this->extension == 'pdf'){
            // Set story item text if the file is an pdf file
            $text = $this->convertPDFFile();
        }
        // Video files
        if(in_array($this->extension, self::$videoFilesExtensions)){
            // Set story item text if the file is an video file
            $text = $this->convertVideoFile();
        }
        // Audio files
        if(in_array($this->extension, self::$audioFilesExtensions)){
            // Set story item text if the file is an video file
            $text = $this->convertAudioFile();
        }
        return $text;
    }

    private function convertImageFile(){

        // Check if it is an image file
        if(!in_array($this->extension, self::$imageFileExtensions)) return "";
        $path = storage_path($this->path.$this->fileName);
        $type = pathinfo($path, PATHINFO_EXTENSION);
        $data = file_get_contents($path);
        $base64 = 'data:image/' . $type . ';base64,' . base64_encode($data);

        return "<img src='".$base64."' alt='".$this->realName."'>";
    }

    private function convertVideoFile(){

        // Check if it is an image file
        if(!in_array($this->extension, self::$videoFilesExtensions)) return "";



        $path = storage_path($this->path.$this->fileName);
        $type = pathinfo($path, PATHINFO_EXTENSION);
        $data = file_get_contents($path);
        $base64 = 'data:video/' . $type . ';base64,' . base64_encode($data);


        return "<video controls><source type=\"video/".$type."\" src=\"".$base64."\"></video>";
    }

    private function convertAudioFile(){

        // Check if it is an image file
        if(!in_array($this->extension, self::$audioFilesExtensions)) return "";



        $path = storage_path($this->path.$this->fileName);
        $type = pathinfo($path, PATHINFO_EXTENSION);
        $data = file_get_contents($path);
        $base64 = 'data:audio/' . $type . ';base64,' . base64_encode($data);


        return "<audio controls><source type=\"audio/".$type."\" src=\"".$base64."\"></audio>";
    }

    private function convertPDFFile(){

        // Check if it is an image file
        if($this->extension != 'pdf') return "";
        if(strtoupper(substr(PHP_OS, 0, 3)) !== 'WIN') return "";
        $source_pdf=storage_path($this->path.$this->fileName);
        $output_folder=storage_path("app/uploads/temp");

        if(!file_exists($output_folder)) mkdir($output_folder, 777);

        $cmd = '"'.storage_path().'\\app\\pdftohtml.exe" "'.$source_pdf.'" "'.$output_folder.'\\'.$this->fileName.'"';
        exec( $cmd, $out, $ret);

        if(file_exists($output_folder.'\\'.$this->fileName.'s.html')){
            $data = file_get_contents($output_folder.'\\'.$this->fileName.'s.html');
            $data = preg_replace_callback ('/src="(.*?)"/i', function($matches) use ($output_folder) {
                $file = $output_folder."\\".$matches[1];
                $type = pathinfo($file, PATHINFO_EXTENSION);
                $data = file_get_contents($file);
                return 'src="data:image/' . $type . ';base64,' . base64_encode($data).'"';
            }, $data);
        }
        // Delete only with this filename
        $files = glob($output_folder.'/'.$this->fileName.'*');
        foreach($files as $file){ // iterate files
            if(is_file($file))
                unlink($file); // delete file
        }
        return $data;
    }


    /**
     * Convert a docx file to text
     * @return bool|string|null
     */
    private function convertDocxFile(){

        // Check if it is a docx file
        if($this->extension != 'docx') return "";
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