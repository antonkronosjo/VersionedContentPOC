export default {
    api: {
        input: {
            target: 'https://localhost:7211/swagger/v1/swagger.json'
        },
        output: {
            target: 'src/api/client.ts',
            client: 'react-query'
        },
    },
};