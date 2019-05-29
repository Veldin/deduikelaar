<?php

namespace App\Http\Controllers\Api;



use App\Feedback;
use App\File;
use App\Http\Controllers\Controller;
use App\Story;
use App\StoryItem;
use Carbon\Carbon;
use http\Env\Response;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Validator;
use Symfony\Component\HttpFoundation\File\UploadedFile;

class StoryController extends Controller
{


    public function getOverview(){

        $data = [];
        $stories = \App\Story::with('feedback')->get();
        $feedbacks = \App\Feedback::with('feedbackItems')->get();

        foreach ($stories as $story){
            $questions = [];
            foreach ($feedbacks as $feedback){
                $answers = [];
                // add each feedbackitem to a story.
                foreach ($feedback->feedbackItems as $feedbackItem){
                    $answers[$feedbackItem->id] = [
                        'feedbackId' => $feedbackItem->id,
                        'answer' => $feedbackItem->feedback,
                        'count' => 0,
                    ];
                }
                // Count the amount of feedback given
                foreach ($story->feedback as $feedbackItem){
                    if($feedbackItem->question->id != $feedback->id) continue;
                    if( isset($answers[$feedbackItem->id]) &&
                        isset($answers[$feedbackItem->id]['count']) ){
                        $answers[$feedbackItem->id]['count'] += 1;
                    }
                }
                // Sort answers
                $answers = array_values($answers);
                usort($answers, function($a, $b) {
                    return $a['count'] - $b['count'];
                });

                // Add feedback to the questions which will be added to the story
                $questions[] = [
                    'question' => $feedback->question,
                    'extraInfo' => $feedback->extraInfo,
                    'feedbackType' => $feedback->feedbackType,
                    'answers' => array_values($answers)
                ];
            }

            // add story to data
            $data[] = [
                'storyId' => $story->id,
                'title' => $story->title,
                'description' => $story->description,
                'icon' => $story->icon,
                'active' => $story->active,
                'feedback' => $questions
            ];

        }

        return  response()->json($data);
    }

    public function getStories(){

        $data = [];
        $stories = Story::with('storyItems')->where('active', 1)->get();

        foreach ($stories as $story){
            $data[] = [
                'storyId' => $story->id,
                'icon' => $story->icon,
                'html' => view('information-piece', compact('story'))->render()
            ];
        }
        return $data;
    }

    public function testStory(){

        $story = Story::with('storyItems')->where('active', 1)->find(1);
        return view('information-piece', compact('story'));
    }


    public function newStory(Request $request){

        $title = $request->get('title');
        $icon = $request->get('icon');
        $description = $request->get('description');
        $texts = $request->get('texts');
        $files = $request->file('files');

        $validation = Validator::make($request->all(), [
            'title' => 'required',
            'icon' => 'required'
        ]);

        if ($validation->fails()) {
            return response()->json([
                'response' => 'failed',
                'errors' => $validation->errors()
            ]);
        }

        $storyData = [
            'title' => $title,
            'icon' => $icon
        ];

        if(strlen($description) > 0){
            $storyData['description'] = $description;
        }


        $story = Story::create($storyData);

        if($texts) {
            if(!is_array($texts)){
                $texts = [$texts];
            }
            foreach ($texts as $text) {
                $storyItem = StoryItem::create([
                    'text' => $text,
                    'storyId' => $story->id
                ]);
            }
        }
        if($files){

            foreach ($files as $file) {
                $f = $file;
                $extension = $f->getClientOriginalExtension();
                $fn = $f->getClientOriginalName();

                $filename = Carbon::now()->format('Ymdhis') . rand(11111111, 99999999) . '.' . $extension;
                $file->storeAs("/uploads/story/", $filename, 'local');

                $storyItem = StoryItem::create([
                    'text' => null,
                    'storyId' => $story->id
                ]);

                $file = File::create([
                    'fileName' => $filename,
                    'realName' => $fn,
                    'fileType' => $f->getMimeType(),
                    'extension' => $extension,
                    'path' => 'app/uploads/story/',
                    'storyItemId' => $storyItem->id
                ]);
                $text = $this->convertDocxFile($file);
                if($text != null){
                    $storyItem->update(['text' => $text]);
                }
            }
        }

        return response()->json([
            'response' => 'success',
            'storyId' => $story->id
        ]);

    }

    public function changeStory(Request $request, $storyId){
        var_dump($storyId);



//        var_dump($request);
    }



    public function deleteStory($storyId){
        $story = Story::find($storyId);
        if($story){
            $story->delete();
            return response()->json([
                'response' => 'success'
            ]);
        }
        return response()->json([
            'response' => 'failed'
        ]);
    }


    public function convertDocxFile(File $file){

        if($file->extension != 'docx') return null;
        //TODO: convert wordt files
//        var_dump( $file->extension );
//
//
        $striped_content = '';
        $content = '';

        $zip = zip_open(storage_path($file->path.$file->fileName));

        if (!$zip || is_numeric($zip)) return false;
        $media = [];
        $relations = "";
        while ($zip_entry = zip_read($zip)) {

            if (zip_entry_open($zip, $zip_entry) == FALSE) continue;
            if(substr(zip_entry_name($zip_entry), 0, 11) == "word/_rels/"){
                $relations = zip_entry_read($zip_entry, zip_entry_filesize($zip_entry));
            }
            if(substr(zip_entry_name($zip_entry), 0,11) == "word/media/"){

                $media[substr(zip_entry_name($zip_entry), 5)] = [
                    'size' => zip_entry_filesize($zip_entry),
                    'data' => base64_encode(zip_entry_read($zip_entry, zip_entry_filesize($zip_entry)))
                ];
//                var_dump(zip_entry_name($zip_entry));
            }
            if (zip_entry_name($zip_entry) != "word/document.xml") continue;

            $content .= zip_entry_read($zip_entry, zip_entry_filesize($zip_entry));

            zip_entry_close($zip_entry);
        }// end while

        zip_close($zip);

        $xmlRel = simplexml_load_string($relations);
//        var_dump($media[0]['name']);
        $relations = [];
        foreach ($xmlRel->Relationship as $relation) {
            $attributes = $relation->attributes();
//            var_dump((string)$attributes->Type);
            if(strpos((string) $attributes->Type, 'image') !== false){
                $media[(string)$attributes->Id] = $media[(string)$attributes->Target];
                unset($media[(string)$attributes->Target]);
            }
        }
//        var_dump($media);

        $xml = simplexml_load_string($content,null, 0, 'w', true);
        preg_match_all('/xmlns:(.*?)="(.*?)"/i', $content, $matches);

        $namespaces = [];
        if(count($matches) > 2){
            foreach ($matches[1] as $i => $v){
                if(isset($matches[2][$i])){
                    $xml->registerXPathNamespace($v, $matches[2][$i]);
                    $namespaces[$v] = true;
                }
            }
        }

        $body = $xml->body;

        $content = "";
        $mediaIndex = 0;
        foreach($body[0] as $key => $value){
            $content .= "<p>";
            if($key == "p"){
                foreach ($value->r as $kkey => $vvalue) {
                    $content .= (string)$vvalue->t;
                }

                $drawing = $this->findDrawing($value);
                if($drawing){
                    $search = $drawing->xpath("//*[local-name()='blip']");
                    if(isset($search[$mediaIndex])){
                        if(isset($search[$mediaIndex]->attributes('r', true)[0])){
                            $id = (string) $search[$mediaIndex]->attributes('r', true)[0];
                            if(isset($media[$id])){
                                $content .= "<img src='data:image/png;base64, ".$media[$id]['data']."'/>";
                            }
                            $mediaIndex++;
                        }
                    }

//                    $this->findPictureId($drawing, array_keys($namespaces));
//                    var_dump();
//                    var_dump($drawing->children('wp', true)[0]->children('a', true)[0]->graphicData->children('pic', true)[0]->blipFill->children('a', true)->blip[0]->attributes('r', true)[0]);
//                    var_dump($drawing->children('x', true));
                }
            }
            $content .= "</p>";
        }
//        var_dump($c->);
//        var_dump($content);
//        $striped_content = $content;
//        var_dump($striped_content);

        return $content;

    }

    private function findDrawing($v){

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

    private function findPictureId($v, $namespaces, $parent = ""){
//        var_dump($parent);
//        var_dump($v->blip);
//        $v->
//        children('wp', true)[0]->
//        children('a', true)[0]->graphicData->
//        children('pic', true)[0]->blipFill->
//        children('a', true)->blip[0]->
//        attributes('r', true)[0]

//        if(isset($v->children('a', true)->blip)){
//            var_dump("got it");
//            return $v->blip[0]->attributes('r', true)[0];
//        }

        foreach ($namespaces as $ns){
            $childs = $v->children($ns, true);

            foreach ($childs as $v){
                var_dump($v->getName());
            }
        }


//        foreach ($namespaces as $namespace => $null){
//
//
//            var_dump("drawing".$parent."->children('".$namespace."', true)");
//            $type = gettype($v->children($namespace, true)[0]);
//            if($type != 'NULL'){
//                $child = $v->children($namespace, true)[0];
//                $n = $child->getName();
//                foreach ($child as $k => $val){
//
//                    $r = $this->findPictureId($val, $namespaces, $parent."->".$n."->".$val->getName());
//                    if($r != null) return $r;
//                }
//                foreach ($namespaces as $ns => $nulll){
//                    if($ns == $namespace) continue;
//
//                    foreach ($child->children($ns, true) as $k => $val){
//
//                        $r = $this->findPictureId($val, $namespaces, $parent."->".$n."->".$val->getName());
//                        if($r != null) return $r;
//                    }
//                }
//            }else{
//                foreach ($v->children($namespace, true) as $k => $val){
////                    var_dump($k);
//                    $r = $this->findPictureId($val, $namespaces, $parent."->".$val->getName());
//                    if($r != null) return $r;
//                }
//            }
//        }
//        foreach ($v as $l => $w) {
//
//
//
//
//            if(isset($w->drawing)){
//                return $w->drawing;
//            }
//            if(count($w) > 0){
//                $d = $this->findDrawing($w);
//                if($d != null) return $d;
//            }
//        }
        return null;
    }



}
