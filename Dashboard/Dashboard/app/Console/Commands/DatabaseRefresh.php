<?php

namespace App\Console\Commands;


use App\Feedback;
use App\FeedbackItem;
use App\StoryFeedback;
use Illuminate\Console\Command;
use Illuminate\Support\Facades\Artisan;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;

class DatabaseRefresh extends Command
{
    /**
     * The name and signature of the console command.
     *
     * @var string
     */
    protected $signature = 'db {first?} {--f=0} {--fill=0}';

    /**
     * The console command description.
     *
     * @var string
     */
    protected $description = 'Refresh or fill database';

    /**
     * Create a new command instance.
     *
     * @return void
     */
    public function __construct()
    {
        parent::__construct();
    }

    /**
     * Execute the console command.
     */
    public function handle()
    {
        $fill = intval($this->option('fill')) + intval($this->option('f'));
        $f = $this->argument('first');
        $refresh = strtolower($f) == 'r' || strtolower($f) == 'refresh';
        $feedback = strtolower($f) == 'feedback';

        if(!$refresh && !$feedback && $fill <= 0){
            echo "\n";
            echo "Use one of these commands:\n";
            echo "\tphp artisan db refresh\n";
            echo "\tphp artisan db --fill=10\n";
            echo "\tphp artisan db refresh fill=10\n";
            echo "\tphp artisan db feedback\n";
        }

        if($refresh){
            echo "\n";
            echo "Refreshing the database\n";
            if(Schema::hasTable('migrations')){
                $migration = DB::table('migrations')->orderBy('batch', 'DESC')->first();

                for($i=0;$i<$migration->batch;$i++){
                    Artisan::call('migrate:rollback');
                }
            }

            Artisan::call('migrate');
        }

        if($feedback){
            echo "\n";
            echo "Refreshing the database\n";
            if(Schema::hasTable('migrations')){
                $migration = DB::table('migrations')->orderBy('batch', 'DESC')->first();

                for($i=0;$i<$migration->batch;$i++){
                    Artisan::call('migrate:rollback');
                }
            }

            Artisan::call('migrate');
            echo "Adding feedback\n";

            $dbs = new \DatabaseSeeder();
            $dbs->addFeedback();
        }

        if($fill > 0){
            echo "\n";
            echo "Filling the database\n";

            $dbs = new \DatabaseSeeder();
            $dbs->run($fill);
        }
    }
}