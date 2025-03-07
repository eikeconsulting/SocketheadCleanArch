import { ActivityIndicator, Platform, StyleSheet, View } from 'react-native';
import React from 'react';
import Colors from '@app/colors';

const Loader = ({ animating = false }: LoaderProps) => {
    if (animating) {
        return (
            <View style={styles.container}>
                <ActivityIndicator color={Colors.primaryColor} animating={animating} size={Platform.OS == "ios" ? "large" : 50} />
            </View>
        );
    }
    return <></>;
}

export default Loader;

const styles = StyleSheet.create({
    container: { zIndex: 100, justifyContent: 'center', backgroundColor: Colors.whiteLayer, ...StyleSheet.absoluteFillObject, }
})

interface LoaderProps {
    animating: boolean
}
