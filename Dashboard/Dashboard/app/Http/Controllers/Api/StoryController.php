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
//        var_dump($request);

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
            foreach ($texts as $num => $text) {
                $storyItem = StoryItem::create([
                    'text' => $text,
                    'storyId' => $story->id
                ]);

                if (isset($files[$num])) {
                    $f = $files[$num];
                    $extension = $f->getClientOriginalExtension();
                    $fn = $f->getClientOriginalName();

                    $filename = Carbon::now()->format('Ymdhis') . rand(11111111, 99999999) . '.' . $extension;
                    $files[$num]->storeAs("/uploads/story/", $filename, 'local');

                    $file = File::create([
                        'fileName' => $filename,
                        'realName' => $fn,
                        'fileType' => $f->getMimeType(),
                        'extension' => $extension,
                        'path' => 'app/uploads/story/',
                        'storyItemId' => $storyItem->id
                    ]);
                    $this->convertDocxFile($file);
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

        while ($zip_entry = zip_read($zip)) {

            if (zip_entry_open($zip, $zip_entry) == FALSE) continue;

            if (zip_entry_name($zip_entry) != "word/document.xml") continue;

            $content .= zip_entry_read($zip_entry, zip_entry_filesize($zip_entry));

            zip_entry_close($zip_entry);
        }// end while

        zip_close($zip);

//        $content = str_replace('</w:r></w:p></w:tc><w:tc>', "", $content);
//        $content = str_replace('</w:r></w:p>', "\r\n", $content);
//        $striped_content = strip_tags($content);
        $body = preg_replace("/\<w\:body\>(.*?)\<\/w\:body\>/im", "test", $content);
//        $c = simplexml_load_string($content);
        var_dump($body);
//        var_dump($content);
        $striped_content = $content;
//        var_dump($striped_content);

        return $striped_content;

    }



}
