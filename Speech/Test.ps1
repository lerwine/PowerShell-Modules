$ParameterNames = @{
    promptBuilder = 'PromptBuilder';
    rate = 'int';
    gender = 'VoiceGender';
    age = 'VoiceAge';
    voice = 'string';
    volume = 'int'
    state = 'object';
    path = 'string';
    formatInfo = 'SpeechAudioFormatInfo';
    audioDestination = 'Stream';
}

$ParamSets = @(
    @('promptBuilder', 'rate', 'voice', 'volume', 'state'),
    @('promptBuilder', 'rate', 'voice', 'volume', 'path', 'formatInfo', 'state'),
    @('promptBuilder', 'rate', 'voice', 'volume', 'audioDestination', 'formatInfo', 'state'),
    @('promptBuilder', 'rate', 'gender', 'age', 'volume', 'state'),
    @('promptBuilder', 'rate', 'gender', 'age', 'volume', 'path', 'formatInfo', 'state'),
    @('promptBuilder', 'rate', 'gender', 'age', 'volume', 'audioDestination', 'formatInfo', 'state'),
    @('promptBuilder', 'rate', 'gender', 'volume', 'state'),
    @('promptBuilder', 'rate', 'gender', 'volume', 'path', 'formatInfo', 'state'),
    @('promptBuilder', 'rate', 'gender', 'volume', 'audioDestination', 'formatInfo', 'state')
) | Sort-Object -Property 'Count';
$Items = @();
$ParamSets | ForEach-Object { $Items = $Items + @(,@($_ | Where-Object { $_ -ne 'volume' })) };
$ParamSets = $ParamSets + $Items;
$Items = @();
$ParamSets | ForEach-Object { $Items = $Items + @(,@($_ | Where-Object { $_ -ne 'rate' })) };
$ParamSets = $ParamSets + $Items;
$Items = @();
$ParamSets | Where-Object { $_ -contains'formatInfo' } | ForEach-Object { $Items = $Items + @(,@($_ | Where-Object { $_ -ne 'formatInfo' })) };
$ParamSets = $ParamSets + $Items;
$ParamSets | Sort-Object -Property 'Count' | ForEach-Object {
    $CodeA = '';
    $CodeB = '';
    if ($_ -contains 'rate') {
        $CodeA += ', int rate';
        $CodeB += ', rate';
    } else {
        $CodeB += ', null';
    }
    if ($_ -contains 'gender') {
        $CodeA += ', VoiceGender gender';
        if ($_ -contains 'age') {
            $CodeA += ', VoiceAge age';
            $CodeB += ', new object[] { gender, age }';
        } else {
            $CodeB += ', gender';
        }
    } else {
        $CodeA += ', string voice';
        $CodeB += ', voice';
    }
    if ($_ -contains 'volume') {
        $CodeA += ', int volume';
        $CodeB += ', volume';
    } else {
        $CodeB += ', null';
    }
    if ($_ -contains 'path') {
        $CodeA += ', string path';
        $CodeB += ', path';
    }
    if ($_ -contains 'audioDestination') {
        $CodeA += ', Stream audioDestination';
        $CodeB += ', audioDestination';
    }
    if ($_ -contains 'formatInfo') {
        $CodeA += ', SpeechAudioFormatInfo formatInfo';
        $CodeB += ', formatInfo';
    } else {
        $CodeB += ', null';
    }
    @'
    
        public void StartSpeechAsync(PromptBuilder promptBuilder{0}, object state)
        {{
            _StartSpeechAsync(promptBuilder{1}, state);
        }}
'@ -f $CodeA, $CodeB;
}