const axios = require('axios');
const {Worker} = require('worker_threads');

const URL = 'https://jsonplaceholder.typicode.com/posts';
const BATCH_SIZE = 25; // Part 1b.
const SERVICE_PATH = './service.js' // Part 1b.

console.log('Starting rest-to-jms component ...');

// Part 1a.
axios.get(URL)
    .then(res => {
        console.log('Status Code: ', res.status);

        const posts = res.data;

        console.log('Processing posts ...');

        // Part 1b.
        const batches = chunkArray(posts, BATCH_SIZE);

        batches.forEach(batch => {
            console.log('Processing batch ...');
            
            // Part 1b. Multi-Threading
            runService(batch);
        });
    })
    .catch(err => {
        console.log('Error: ', err.message)
    });

// Breaks up array into chunks
// Reference: https://stackoverflow.com/questions/8495687/split-array-into-chunks/24782004
const chunkArray = (array, chunk_size) => array
    .map((_, i, all) => all.slice(i*chunk_size, (i+1)*chunk_size))
    .filter(x => x.length);

// Handle multi-threading with Worker threads (wrapped as a Promise)
// Reference: https://blog.logrocket.com/node-js-multithreading-what-are-worker-threads-and-why-do-they-matter-48ab102f8b10/
function runService(workerData) {
    return new Promise((resolve, reject) => {
        const worker = new Worker(SERVICE_PATH, { workerData });
        worker.on('message', resolve);
        worker.on('error', reject);
        worker.on('exit', (code) => {
        if (code !== 0)
            reject(new Error(`Worker stopped with exit code ${code}`));
        })
    })
}