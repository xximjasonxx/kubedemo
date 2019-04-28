
import React from 'react';
import { connect } from 'react-redux';
import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr';

class MainLandingComponent extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            hubConnection: null,
            stockPrices: {}
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
            this.setState({
                stockPrices: Object.assign({}, this.state.stockPrices, {
                    [stockPrice.symbol]: stockPrice.newPrice
                })
            });
        });
    }

    render() {
        const { stockPrices } = this.state;
        return (
            <div>
                <h2>Stock Prices</h2>
                {Object.keys(stockPrices).map((key) => <h3>{key} - {stockPrices[key]}</h3>)}
            </div>
        );
    }
};

const mapStateToProps = (state) => {
    return {};
};

export default connect(mapStateToProps)(MainLandingComponent);
