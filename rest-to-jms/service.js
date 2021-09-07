const {workerData, parentPort} = require('worker_threads');
const stompit = require('stompit');

const connectOptions = {
  'host': 'localhost',
  'port': 61613,
  'connectHeaders':{
    'host': '/',
    'login': 'admin',
    'passcode': 'admin',
    'heart-beat': '5000,5000'
  }
};

// Part 1c. Process each batch of posts and send to JMS queue
workerData.forEach(post => {
    console.log(post.id);
    sendJmsMessage(post);
});

parentPort.postMessage({processed:workerData});

function sendJmsMessage(message) {
    stompit.connect(connectOptions, function(error, client) {
  
        if (error) {
          console.log('Connection error: ' + error.message);
          return;
        }
        
        const sendHeaders = {
          'destination': '/queue/posts',
          'content-type': 'text/plain'
        };
        
        const frame = client.send(sendHeaders);

        frame.write(JSON.stringify(message));
        frame.end();

        client.disconnect();
    });
}