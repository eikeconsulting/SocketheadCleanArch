import { StyleSheet, TouchableOpacity, View } from 'react-native';
import React from 'react';
import Text from '@app/components/text';
import Loader from '@app/components/loader';
import Colors from '@app/colors';

const Button = ({ label = 'Default Label', onPress = () => { }, disabled = false, containerStyles = {}, textStyles = {}, loading = false }: ButtonProps) => {
    if (loading) {
        return (
            <View style={{ borderRadius: 12, paddingVertical: 20, ...containerStyles }}>
                <Loader animating />
            </View>
        );
    }
    return (
        <TouchableOpacity disabled={disabled} onPress={() => onPress()} style={{ backgroundColor: disabled ? Colors.blueGrey : Colors.primaryColor, borderRadius: 12, paddingVertical: 20, ...containerStyles }}>
            <Text onPress={() => null} style={{ color: Colors.white, textAlign: 'center', fontSize: 14, ...textStyles }}>{label}</Text>
        </TouchableOpacity>
    );
}

export default Button;

const style = StyleSheet.create({

})

interface ButtonProps {
    label: string,
    onPress: Function,
    disabled: boolean,
    containerStyles: object
    textStyles: object,
    loading: boolean
};
