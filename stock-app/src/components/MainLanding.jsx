
import React from 'react';
import { connect } from 'react-redux';
import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr';

class MainLandingComponent extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            hubConnection: null
        };

        this.handleConnectionStart = this.handleConnectionStart.bind(this);
    }

    componentDidMount = () => {
        const hubConnection = new HubConnectionBuilder()
            .withUrl("http://192.168.99.100:31000/prices")
            .configureLogging(LogLevel.Information)
            .build();

        this.setState({ hubConnection }, () => {
            this.state.hubConnection
                .start()
                .then(this.handleConnectionStart)
                .catch(err => {
                        console.log('Connection error: ' + err);
                    });
        });
    }

    handleConnectionStart() {
        console.log("Connection established");
        this.state.hubConnection.on('ReceiveStockPrice', (stockPrice) => {
            console.log(stockPrice);
        });
    }

    render() {
        return (
            <div>Hello World</div>
        );
    }
};

const mapStateToProps = (state) => {
    return {};
};

export default connect(mapStateToProps)(MainLandingComponent);
