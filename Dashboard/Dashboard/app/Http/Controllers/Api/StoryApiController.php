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

class StoryApiController extends Controller
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

    public function getStory($storyId){

        $data = [];

        // Get story
        $story = Story::with('storyItems', 'storyItems.file', 'feedback', 'feedback.question')->find($storyId);


        // Return failed response when story couldn't be found
        if(!$story){
            return response()->json([
                'response' => 'failed',
                'errors' => [
                    'Story not found'
                ]
            ]);
        }

        // Set data to return
        $data['id'] = $story->id;
        $data['title'] = $story->title;
        $data['icon'] = $story->title;
        $data['description'] = $story->title;
        $data['texts'] = [];

        // Add story items
        foreach ($story->storyItems as $storyItem){
            $file = null;
            if($storyItem->file){
                $f = $storyItem->file;
                $file = [
                    'id' => $f->id,
                    'filename' => $f->fileName,
                    'path' => $f->path,
                    'fileType' => $f->fileType,
                    'download' => 'api/v1/file/'.$f->id
                ];
            }
            $data['texts'][] = [
                'id' => $storyItem->id,
                'text' => $storyItem->text,
                'file' => $file
            ];
        }

        // Add feedback
        $data['feedbackItems'] = $story->allFeedback();

        return response()->json($data);
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
        return response()->json($data);
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

        $story = Story::with('storyItems', 'storyItems.file')->find($storyId);
        if(!$story){
            return response()->json([
                'response' => 'failed',
                'errors' => [
                    'Story not found'
                ]
            ]);
        }

        $newData = [];

        if($request->has('title')){
            $newData['title'] = $request->get('title');
        }
        if($request->has('icon')){
            $newData['icon'] = $request->get('icon');
        }
        if($request->has('description')){
            $newData['description'] = $request->get('description');
        }

        // TODO change texts and files


//        $title = $request->get('title');
//        $icon = $request->get('icon');
//        $description = $request->get('description');
//        $texts = $request->get('texts');
//        $files = $request->file('files');



        return response()->json([
            'response' => 'success'
        ]);
    }



    public function deleteStory($storyId){
        $story = Story::with('storyItems', 'storyItems.file')->find($storyId);
        if($story){
            foreach ($story->storyItems as $storyItem){
                if(isset($storyItem->file)){
                    $file = storage_path($storyItem->file->path.$storyItem->file->fileName);
                    if(file_exists($file)){
                        unlink($file);
                    }
                }
            }
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

        $relations = [];
        foreach ($xmlRel->Relationship as $relation) {
            $attributes = $relation->attributes();
//            var_dump((string)$attributes->Type);
            if(strpos((string) $attributes->Type, 'image') !== false){
                $media[(string)$attributes->Id] = $media[(string)$attributes->Target];
                unset($media[(string)$attributes->Target]);
            }
        }

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
                }
            }
            $content .= "</p>";
        }

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


}
