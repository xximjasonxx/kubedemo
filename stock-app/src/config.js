
const local = {
    API_HOSTNAME: "http://192.168.99.100:31000"
};

const cluster = {
    API_HOSTNAME: "http://stockapi-service.kube-demo:80"
}

const config = process.env.REACT_APP_STAGE === 'cluster' ? cluster : local;
console.log(config);

export default config;