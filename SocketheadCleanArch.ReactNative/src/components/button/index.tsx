import { TouchableOpacity, View } from 'react-native'
import React from 'react'
import Text from '../text'
import Colors from '../../colors'
import Loader from '../loader'

const Button = ({ label = 'Default Label', onPress = () => { }, disabled = false, containerStyles = {}, textStyles = {}, loading = false }: ButtonProps) => {
    if (loading) {
        return (
            <View style={{ borderRadius: 12, paddingVertical: 20, ...containerStyles }}>
                <Loader animating />
            </View>
        )
    } else {
        return (
            <TouchableOpacity disabled={disabled} onPress={() => onPress()} style={{ backgroundColor: disabled ? Colors.blueGrey : Colors.primaryColor, borderRadius: 12, paddingVertical: 20, ...containerStyles }}>
                <Text numberOfLines={0} onPress={() => null} style={{ color: Colors.white, textAlign: 'center', fontSize: 14, ...textStyles }}>{label}</Text>
            </TouchableOpacity>
        )
    }
}

export default Button

interface ButtonProps {
    label: string,
    onPress: Function,
    disabled: boolean,
    containerStyles: object
    textStyles: object,
    loading: boolean
}